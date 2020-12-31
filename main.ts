function moveForward () {
    basic.showArrow(ArrowNames.East)
    pins.servoWritePin(AnalogPin.P0, 180)
    pins.servoWritePin(AnalogPin.P1, 0)
}
bluetooth.onBluetoothConnected(function () {
    basic.showIcon(IconNames.Yes)
})
function stop () {
    basic.showIcon(IconNames.Square)
    ContinuousServo.turn_off_motor(DigitalPin.P0)
    ContinuousServo.turn_off_motor(DigitalPin.P1)
}
bluetooth.onBluetoothDisconnected(function () {
    basic.showIcon(IconNames.No)
})
input.onButtonPressed(Button.A, function () {
    moveForward()
})
bluetooth.onUartDataReceived(serial.delimiters(Delimiters.NewLine), function () {
    uartData = bluetooth.uartReadUntil(serial.delimiters(Delimiters.NewLine))
    basic.showString(uartData)
    if (uartData.includes("M") || uartData.includes("m")) {
        moveForward()
    } else if (uartData.includes("B") || uartData.includes("b")) {
        moveBackward()
    } else if (uartData.includes("S") || uartData.includes("s")) {
        stop()
    } else {
        basic.showString("INVALID COMMAND")
    }
})
input.onButtonPressed(Button.AB, function () {
    stop()
})
input.onButtonPressed(Button.B, function () {
    moveBackward()
})
function moveBackward () {
    basic.showArrow(ArrowNames.West)
    pins.servoWritePin(AnalogPin.P1, 180)
    pins.servoWritePin(AnalogPin.P0, 0)
}
let uartData = ""
bluetooth.startUartService()
