import os
import time
import cv2
from deepface import DeepFace

os.environ["CUDA_VISIBLE_DEVICES"] = "-1"

face_classifier = cv2.CascadeClassifier()
face_classifier.load(cv2.samples.findFile("haarcascade_frontalface_default.xml"))

cap = cv2.VideoCapture(0)
last_analyze_time = time.time()
emotions_list = {
    'angry' : 0,
    'disgust' : 0,
    'fear' : 0,
    'happy' : 0,
    'sad' : 0 ,
    'surprise' : 0,
    'neutral' : 0
} 
max_key = ""
while True:
    ret, frame = cap.read()
    frame_gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    faces = face_classifier.detectMultiScale(frame_gray)
    
    for face in faces:
        x, y, w, h = face
        # Validate face size
        min_face_size = 100  # Adjust the minimum face size as needed
        if w > min_face_size and h > min_face_size:
            frame = cv2.rectangle(frame, (x, y), (x+w, y+h), color=(255, 0, 0), thickness=1)
            # Delay for 1 second before the next analyze
            current_time = time.time()
            response = DeepFace.analyze(frame[y:y+h, x:x+w], actions=("emotion",), enforce_detection=False)
            if response["dominant_emotion"] == "angry" : 
                emotions_list['angry'] += 1
            elif response["dominant_emotion"] == "disgust" :
                emotions_list['disgust'] += 1
            elif  response["dominant_emotion"] == "fear" :
                emotions_list['fear'] += 1
            elif  response["dominant_emotion"] == "happy" :
                emotions_list['happy'] += 1
            elif  response["dominant_emotion"] == "sad" :
                emotions_list['happy'] += 1
            elif  response["dominant_emotion"] == "surprise" :
                emotions_list['surprise'] += 1
            elif  response["dominant_emotion"] == "neutral" :
                emotions_list['neutral'] += 1      
            
            
            if current_time - last_analyze_time >= 3:
                max_key = max(emotions_list, key=emotions_list.get)
                print(max_key)
                print(emotions_list)
                emotions_list = {
                    'angry' : 0,
                    'disgust' : 0,
                    'fear' : 0,
                    'happy' : 0,
                    'sad' : 0 ,
                    'surprise' : 0,
                    'neutral' : 0
                } 
                last_analyze_time = current_time
