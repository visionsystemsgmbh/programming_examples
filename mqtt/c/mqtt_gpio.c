#include <stdio.h>
#include <stdbool.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <pthread.h>
#include <onrisc.h>
#include <MQTTClient.h>

#define QOS         1
#define TIMEOUT     10000L

bool stop_thread = false;

void on_delivery(void *context, MQTTClient_deliveryToken dt)
{
	printf("Message with token value %d delivery confirmed\n", dt);
}

void *read_digital_input_thread(void *data) {
	MQTTClient client = data;
	uint32_t old_value = 0;
	bool first_run = true;
	
	while(!stop_thread) {
		int i;
		onrisc_gpios_t gpios;
		
		/* read digital input status */
		if (onrisc_gpio_get_value(&gpios) == EXIT_FAILURE) {
			printf("Failed to get GPIO values\n");
			exit(EXIT_FAILURE);
		}

		for (i = 0; i < 4; i++) {
			char buf[32];
			int rc;
			MQTTClient_message pubmsg = MQTTClient_message_initializer;
			MQTTClient_deliveryToken token;
			pubmsg.payloadlen = 1;
			pubmsg.qos = QOS;
			pubmsg.retained = 0;
			
			/* continue if an input hasn't changed*/
			if (((gpios.value & (1 << i)) == (old_value & (1 << i))) && !first_run)
				continue;

			if (gpios.value & (1 << i)) {
				pubmsg.payload = "1";
			} else {
				pubmsg.payload = "0";
			}
			
			/* publish input state */
			sprintf(buf, "onrisc/gpio/input/%d", i);
			MQTTClient_publishMessage(client, buf, &pubmsg, &token);
			printf("Waiting for up to %d seconds for publication of %s\n"
			        "on topic %s for client with ClientID: %s\n",
				(int)(TIMEOUT/1000), (char *)pubmsg.payload, buf, "Baltos");

			/* check publication state */
			rc = MQTTClient_waitForCompletion(client, token, TIMEOUT);
			printf("Message with delivery token %d delivered\n", token);
		}
		
		first_run = false;
		old_value = gpios.value;
		
		sleep(1);
	}

	return NULL;
}

int on_message(void *context, char *topicName, int topicLen,
	     MQTTClient_message * message)
{
	int i;
	char *payloadptr;
	long int gpio_val;
	onrisc_gpios_t gpios;
	char buf[32];

	memcpy(buf, message->payload, message->payloadlen);
	buf[message->payloadlen] = '\0';
	gpio_val = strtol(buf, NULL, 10);
	if (!strcmp(topicName, "onrisc/gpio/output/0")) {
		printf("Switching GPIO0 to %ld\n", gpio_val);
		gpios.mask = 0x10;
		gpios.value = gpio_val ? 0x10 : 0x00;
	} else if  (!strcmp(topicName, "onrisc/gpio/output/1")) {
		printf("Switching GPIO1 to %ld\n", gpio_val);
		gpios.mask = 0x20;
		gpios.value = gpio_val ? 0x20 : 0x00;
	} else if  (!strcmp(topicName, "onrisc/gpio/output/2")) {
		printf("Switching GPIO2 to %ld\n", gpio_val);
		gpios.mask = 0x40;
		gpios.value = gpio_val ? 0x40 : 0x00;
	} else if  (!strcmp(topicName, "onrisc/gpio/output/3")) {
		printf("Switching GPIO3 to %ld\n", gpio_val);
		gpios.mask = 0x80;
		gpios.value = gpio_val ? 0x80 : 0x00;
	}
	if (onrisc_gpio_set_value(&gpios) == EXIT_FAILURE) {
		printf("Failed to set GPIOs\n");
		return(EXIT_FAILURE);
	}
	MQTTClient_freeMessage(&message);
	MQTTClient_free(topicName);

	return 1;
}

void on_connection_lost(void *context, char *cause)
{
	printf("\nConnection lost\n");
	printf("     cause: %s\n", cause);
}

int main(int argc, char *argv[])
{
	MQTTClient client;
	MQTTClient_connectOptions conn_opts =
	    MQTTClient_connectOptions_initializer;
	int rc = EXIT_FAILURE;
	int ch;
	uint16_t mqtt_port = 1883;
	char *buf;
	pthread_t di_thread;

	/* handle command line parameter: IP address and TCP port */
	if (argc < 2 || argc > 3) {
		printf("Please specify IP address "
		       "and/or TCP port number of the MQTT broker\n");
		return EXIT_FAILURE;
	}

	if (argc == 3) {
		mqtt_port = atoi(argv[2]);
	}

	buf = malloc(strlen(argv[1]) + 13);
	if (!buf) {
		printf("Failed to allocate buffer\n");
		exit(EXIT_FAILURE);
	}

	sprintf(buf, "tcp://%s:%d", argv[1], mqtt_port);

	/* initialize libonrisc */
	if (onrisc_init(NULL) != EXIT_SUCCESS) {
		printf("Failed to initialize libonrisc\n");
		goto error;
	}

	/* connect to the MQTT broker */
	MQTTClient_create(&client, buf, "Baltos",
			  MQTTCLIENT_PERSISTENCE_NONE, NULL);
	conn_opts.keepAliveInterval = 20;
	conn_opts.cleansession = 1;
	MQTTClient_setCallbacks(client, NULL, on_connection_lost, on_message, on_delivery);
	if ((rc = MQTTClient_connect(client, &conn_opts)) != MQTTCLIENT_SUCCESS) {
		printf("Failed to connect, return code %d\n", rc);
		goto error;
	}

	MQTTClient_subscribe(client, "onrisc/gpio/output/#", QOS);

	/* start digital input handling thread */
	if(pthread_create(&di_thread, NULL, read_digital_input_thread, client)) {
		printf("Error creating thread\n");
		goto error;
	}

	/* wait for 'Q' or 'q' to terminate the program */
	do {
		ch = getchar();
	} while (ch != 'Q' && ch != 'q');

	stop_thread = true;

	pthread_join(di_thread, NULL);

	rc = EXIT_SUCCESS;

error:
	/* clean up */
	free(buf);
	MQTTClient_disconnect(client, 10000);
	MQTTClient_destroy(&client);

	return rc;
}
