import time
import zmq

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")

while True:
    
    message = socket.recv()
    print("Messaggio ricevuto: %s" % message)

    time.sleep(1)

    reversedMessage = message[::-1]

    socket.send(bytes(reversedMessage))