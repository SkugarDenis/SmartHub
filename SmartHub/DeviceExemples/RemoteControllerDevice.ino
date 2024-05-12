/*
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
#include <IRremoteESP8266.h>
#include <IRsend.h>


ESP8266WiFiMulti WiFiMulti;
WebSocketsClient webSocket;

const uint16_t kIrLed = 4;  // ESP8266 GPIO. Пин по умолчанию: 4 (D2).
IRsend irsend(kIrLed);

String secretKey = "6028340d-fd0d-46aa-8b1b-23c162f4161c";
String Id = "1572fdd9-81b7-4af4-8d57-3f13730b1dd7";
int deviceType = 2;

uint16_t buttonON[201] = {9100,4392,664,488,624,508,620,516,612,520,612,524,612,520,612,520,612,520,612,1628,612,1628,600,1640,612,1628,608,1632,608,1632,608,1632,608,524,608,1632,608,524,584,1656,584,548,584,1656,584,548,588,544,588,568,564,568,564,1656,584,572,560,1656,584,572,560,1676,564,1676,564,1676,564,40828,9044,2208,612};
uint16_t buttonAddVolume[199] = {9100,4388,664,488,640,492,616,520,612,536,596,544,588,544,588,548,588,544,588,1648,592,1648,588,1652,588,1652,588,1652,588,1652,588,1652,588,544,588,1652,588,1652,584,548,588,1652,584,1656,584,548,588,544,588,548,584,548,568,564,588,1652,564,568,564,572,560,1680,560,1680,560,1680,560};
uint16_t buttonRemoveVolume[199] = {9100,4388,664,468,664,488,620,516,612,524,608,524,612,520,612,520,612,520,612,1652,588,1644,596,1628,612,1652,588,1652,588,1652,588,1652,588,544,588,544,588,1652,588,548,584,1652,588,1652,588,544,588,548,588,544,588,1652,588,544,588,1652,588,544,592,544,588,1652,588,1648,592,1648,588,40812,9048,2232,584};

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
            ClickButton(reinterpret_cast<char*>(payload));
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

void ClickButton(char* str)
{
    Serial.print("Received command: ");
    Serial.println(str);

    // Проверяем, является ли переданная команда "power"
    if (strncmp(str, "{\"ItefaceName\":null,\"dataType\":1,\"Value\":\"power\"}", 54) == 0) {
        Serial.println("Executing power command");
        irsend.sendRaw(buttonON, 199, 38);
    }
    // Проверяем, является ли переданная команда "volumeup"
    else if (strncmp(str, "{\"ItefaceName\":null,\"dataType\":1,\"Value\":\"volumeup\"}", 57) == 0) {
        Serial.println("Executing volume up command");
        irsend.sendRaw(buttonAddVolume, 199, 38);
    }
    // Проверяем, является ли переданная команда "volumedown"
    else if (strncmp(str, "{\"ItefaceName\":null,\"dataType\":1,\"Value\":\"volumedown\"}", 61) == 0) {
        Serial.println("Executing volume down command");
        irsend.sendRaw(buttonRemoveVolume, 199, 38);
    }
    // Если значение "Value" не совпадает ни с одним из ожидаемых значений
    else {
        Serial.println("Unknown command");
    }
}

void setup() {
    Serial.begin(115200);
    irsend.begin();
    Serial.print("Start");
    WiFiMulti.addAP("MTS_Router_041481", "89720585");

    WiFi.disconnect();
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