import face_recognition
import cv2
import time
import zmq

from apscheduler.schedulers.background import BackgroundScheduler

sched = BackgroundScheduler()

sched.start()

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")

label = None

message = socket.recv().decode('utf-8')
print("Messaggio ricevuto: %s" % message)

webcam = cv2.VideoCapture(0)

image_file = message.rsplit(" ", 1)[0]
target_image = face_recognition.load_image_file(image_file)
target_encoding = face_recognition.face_encodings(target_image)[0]

print("Image Loaded. 128-dimension Face Encoding Generated. \n")

target_name = message.rsplit(" ", 1)[1]

process_this_frame = True

responseSent = False


def sendResponseToServer(authenticated):
    global responseSent
    if responseSent:
        return
    
    if authenticated:
        socket.send(bytes("Authorized " + label, encoding='utf8'))
    else:
        socket.send(bytes("Denied", encoding='utf8'))

    responseSent = True

    time.sleep(1)
    exit()


def sendFailedResponseToServer():
    sendResponseToServer(False)


sched.add_job(sendFailedResponseToServer, "interval", seconds=7)

while True:
    ret, frame = webcam.read()

    small_frame = cv2.resize(frame, None, fx=0.20, fy=0.20)
    rgb_small_frame = cv2.cvtColor(small_frame, 4)

    if process_this_frame:

        face_locations = face_recognition.face_locations(rgb_small_frame)
        frame_encodings = face_recognition.face_encodings(rgb_small_frame)

        if frame_encodings:
            frame_face_encoding = frame_encodings[0]
            match = face_recognition.compare_faces([target_encoding], frame_face_encoding)[0]
            label = target_name if match else "Unknown"

    process_this_frame = not process_this_frame

    if face_locations:
        top, right, bottom, left = face_locations[0]

        top *= 5
        right *= 5
        bottom *= 5
        left *= 5

        cv2.rectangle(frame, (left, top), (right, bottom), (0, 255, 0), 2)

        cv2.rectangle(frame, (left, bottom - 30), (right, bottom), (0, 255, 0), cv2.FILLED)
        label_font = cv2.FONT_HERSHEY_DUPLEX
        cv2.putText(frame, label, (left + 6, bottom - 6), label_font, 0.8, (255, 255, 255), 1)

        if label is not None:
            sendResponseToServer(True)

    cv2.imshow("Video Feed", frame)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

webcam.release()
cv2.destroyAllWindows()
