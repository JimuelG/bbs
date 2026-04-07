import requests
import time
import subprocess
import urllib3

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

WINDOWS_IP = "10.179.125.128"
BASE_URL = f"https://{WINDOWS_IP}:5001/api/announcement"

print(f"BBS Listener Active. Pointing to {BASE_URL")

def play_audio(data):
  try:
    title = data.get("title", "untitled")
    path = data.get("audioUrl")

    if path is None:
      print(f"[!] Error: Announcement '{title}' has no Audio URL.")
      return
    if path.startswith('/'): path = path[1:]
    audio_url = f"https://{WINDOWS_IP}:5001/{path}"

    print(f"\n[!] TRIGGERED: {title}")
    print(f"[*] Downloading: {audio_url}")

    r = requests.get(audio_url, verify=False, timeout=10)
    with open("current_announcement.mp3", "wb") as f:
        f.write(r.content)

    subprocess.run(["mpg321", "current_announcement.mp3"])

    requests.post(f"{BASE_URL}/mark-played/{data.get('id')}", verify=False)

  except Exception as e:
    print(f"Error playing audio: {e}")

print("--- Barangay RPi Multi-Trigger System Active ---")

while True:
    try:
        try:
          requests.post(f"{BASE_URL}/heartbeat", verify=False, timeout=2)
        except:
            print("!", end="", flush=True)
        emergency = requests.get(f"{BASE_URL}/latest", verify=False, timeout=3)
        if emergency.status_code == 200:
            play_audio(emergency.json())
            continue

        manual = requests.get(f"{BASE_URL}/manual-trigger", verify=False, timeout=3)
        if manual.status_code == 200:
            play_audio(manual_json())
            continue

        scheduled = requests.get(f"{BASE_URL}/latest", verify=False, timeout=3)
        if scheduled.status_code == 200:
            play_audio(scheduled.json())

    except Exception as e:
        print(f"Connection error: {e}")
    
    print(".", end="", flush=True)
    time.sleep(5)
