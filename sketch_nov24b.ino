#include <Servo.h>

Servo servo;                // Objeto para controlar el servomotor
const int servoPin = 9;     // Pin donde está conectado el servomotor
const int ledPin = 8;       // Pin donde está conectado el LED
const int buttonPin = 7;    // Pin donde está conectado el botón

unsigned long previousMillis = 0; // Almacenar el tiempo anterior para el servo
const unsigned long interval = 5200; // Intervalo de 7 segundos para el servo

int currentAngle = 0;  // Ángulo actual del servo
int step = 180;        // Ángulo hacia donde cambiará el servo (0 a 180 grados)

bool servoActive = false; // Variable para controlar si el servo está activo
bool buttonState = HIGH;  // Estado inicial del botón
bool lastButtonState = HIGH; // Estado previo del botón (para evitar repetición)

void setup() {
    pinMode(ledPin, OUTPUT);    // Configurar el LED como salida
    pinMode(buttonPin, INPUT_PULLUP); // Configurar el botón como entrada con resistencia interna
    servo.attach(servoPin);     // Conectar el servo al pin 9
    Serial.begin(9600);         // Iniciar la comunicación serial
    Serial.println("Arduino Listo");
}

void loop() {
    // Movimiento automático del servo si está activo
    if (servoActive) {
        unsigned long currentMillis = millis();
        if (currentMillis - previousMillis >= interval) {
            previousMillis = currentMillis; // Actualizar el tiempo previo

            // Cambiar la dirección del servo
            currentAngle = (currentAngle == 0) ? 180 : 0; // Alternar entre 0° y 180°
            servo.write(currentAngle);                   // Mover el servo al nuevo ángulo
            Serial.println("Servo movido a: " + String(currentAngle) + " grados");
        }
    }

    // Leer el estado del botón
    buttonState = digitalRead(buttonPin);
    if (buttonState == LOW && lastButtonState == HIGH) {
        // Botón presionado
        Serial.println("BUTTON_PRESSED"); // Enviar el comando "FIRE" a Unity
        delay(300); // Antirrebote
    }
    lastButtonState = buttonState;

    // Escuchar comandos desde Unity
    if (Serial.available() > 0) {
        String command = Serial.readStringUntil('\n'); // Leer el comando hasta un salto de línea
        command.trim(); // Eliminar espacios en blanco adicionales
        Serial.println("Recibido: " + command); // Confirmar el comando recibido

        if (command == "FIRE") {
            digitalWrite(ledPin, HIGH); // Enciende el LED
            delay(500);                // Mantén el LED encendido por 500 ms
            digitalWrite(ledPin, LOW); // Apaga el LED
            Serial.println("Comando de disparo ejecutado: LED encendido");
        } else if (command == "START_SERVO") {
            servoActive = true; // Activar el movimiento del servo
            Serial.println("Movimiento del servo activado");
        } else if (command == "STOP_SERVO") {
            servoActive = false; // Desactivar el movimiento del servo
            Serial.println("Movimiento del servo desactivado");
        } else if (command.startsWith("MOVE:")) {
            int angle = command.substring(5).toInt(); // Extraer ángulo del comando
            servo.write(angle);                      // Mover el servo al ángulo especificado
            Serial.println("Servo movido a: " + String(angle) + " grados");
        } else {
            Serial.println("Comando desconocido: " + command);
        }
    }
}



