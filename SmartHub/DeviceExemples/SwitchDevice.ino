﻿/*
 *
 * ESP8266 only
 *
 */

#include <Arduino.h>

#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>
#include <WebSocketsClient.h>
#include <Hash.h>
#include <Crypto.h>
#include <MD5Builder.h>


ESP8266WiFiMulti WiFiMulti;
WebSocketsClient webSocket;

String secretKey = "6028340d-fd0d-46aa-8b1b-23c162f4161c";
String Id = "a31b3317-64e7-4349-94fc-e38b65492779";
int deviceType = 1;

void webSocketEvent(WStype_t type, uint8_t * payload, size_t length) {
    int jsonLength;
    char json[256];

    switch(type) {
        case WStype_DISCONNECTED:
            Serial.println("[WSc] Disconnected!");
            break;
        case WStype_CONNECTED:
            Serial.println("[WSc] Connected!");
            // Авторизуемся при коннекте
            char signature[33]; // MD5 хэш состоит из 32 символов плюс нулевой символ

            char requestId[37];
            generateGUID(requestId);

            // Генерируем подпись MD5
            generateMD5Signature(secretKey, Id + requestId, signature);
            // Вычисляем размер JSON строки
            jsonLength = snprintf(NULL, 0, "{\"ExtensionDeviceId\": \"%s\", \"requestId\": \"%s\", \"deviceType\": \"%d\", \"typeRequest\": \"%d\", \"requestObject\": {\"Signature\": \"%s\"}}", Id.c_str(), requestId, deviceType, 1, signature);
            // Форматируем JSON строку
            snprintf(json, sizeof(json), "{\"ExtensionDeviceId\": \"%s\", \"requestId\": \"%s\", \"deviceType\": \"%d\", \"typeRequest\": \"%d\", \"requestObject\": {\"Signature\": \"%s\"}}", Id.c_str(), requestId, deviceType, 1, signature);

            webSocket.sendTXT(json);
            break;
        case WStype_TEXT:
            // Выводим текстовое сообщение в последовательный порт
            Serial.write(payload, length);
            Serial.println();
            break;
        case WStype_BIN:
            Serial.printf("[WSc] Received binary data. Length: %u\n", length);
            // Выводим данные в шестнадцатеричном формате
            for (size_t i = 0; i < length; i++) {
                Serial.print(payload[i], HEX);
                Serial.print(" ");
            }
            Serial.println();
            break;
    }
}

void setup() {
    Serial.begin(115200);
    Serial.print("Start");
    WiFiMulti.addAP("MTS_Router_041481", "89720585");

    //WiFi.disconnect();
    while(WiFiMulti.run() != WL_CONNECTED) {
        delay(100);
    }

    webSocket.beginSSL("192.168.1.192", 7150, "/devices");
    webSocket.onEvent(webSocketEvent);

}

void loop() {
    webSocket.loop();
}

void generateMD5Signature(String key, String data, char* signature) {
  MD5Builder md5;
  md5.begin();
  md5.add(key);
  md5.add(data);
  md5.calculate();
  md5.getChars(signature);
}

void generateGUID(char* guid) {
  for (int i = 0; i < 32; i++) {
    if (i == 8 || i == 12 || i == 16 || i == 20) {
      *guid++ = '-';
    }
    snprintf(guid, 2, "%X", random(15)); // Записываем два символа HEX числа
    guid += 2;
  }
  *guid = '\0'; // Добавляем завершающий нуль
}