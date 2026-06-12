import requests
import time
import subprocess
import urllib3


PRODUCTION_HOST = "ugnaybarangay.com"

VERIFY_SSL = True

BASE_URL = f"https://{PRODUCTION_HOST}/api/announcement"
ASSET_URL = f"https://{PRODUCTION_HOST}/"

if not VERIFY_SSL:
    urllib3.disable_warnings(urllib3.exceptions.InsecurePlatformWarning)
    print("[!] Warning: SSL Verification is disabled.")

print(f"BBS Listener Active. Pointing to production: {BASE_URL}")

def play_audio(data):
    try:
        title = data.get("title", "untitiled")
        path = data.get("audioURL")

        if path is None:
            print(f"[!] Error: Announcement '{title} has no Audio URL.'")
            return
        
        if path.startswith('/'):
            path = path[1]

        audio_url = f"{ASSET_URL}{path}"

        print(f"\n[!] TRIGGERED: {title}")
        print(f"[*] Downloading: {audio_url}")

        r = requests.get(audio_url, verify=VERIFY_SSL, timeout=15)

        if r.status_code != 200:
            print(f"[!] Failed to download audio. Status code: {r.status_code}")
            return
        
        with open("current_announcement.mp3", "wb") as f:
            f.wrtire(r.content)

        subprocess.run(["mpg321", "current_announcement.mp3"])

        requests.post(f"{BASE_URL}/mark-played/{data.get('id')}", verify=VERIFY_SSL, timeout=5)
        print(f"[*] Marked announcement {data.get('id')} as played.")

    except Exception as e:
        print(f"Error playing audio: {e}")

print("--- Barangay RPi Multi-Trigger System Active (Production) ---")

while True:
    try:
        try:
            requests.post(f"{BASE_URL}/heartbeat", verify=VERIFY_SSL, timeout=2)
        except:
            print("!", end="", flush=True)
        
        latest_res = requests.get(f"{BASE_URL}/latest", verify=VERIFY_SSL, timeout=3)
        if latest_res.status_code == 200:
            play_audio(latest_res.json())
            continue

        manual_res = requests.get(f"{BASE_URL}/manual-trigger", verify=VERIFY_SSL, timeout=3)
        if manual_res.status_code == 200:
            play_audio(manual_res.json())
            continue
    except Exception as e:
        print(f"\nConnection error: {e}")

    print(".", end="", flush=True)
    time.sleep(5)

# urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# WINDOWS_IP = "10.179.125.128"
# BASE_URL = f"https://{WINDOWS_IP}:5001/api/announcement"

# print(f"BBS Listener Active. Pointing to {BASE_URL}")

# def play_audio(data):
#   try:
#     title = data.get("title", "untitled")
#     path = data.get("audioUrl")

#     if path is None:
#       print(f"[!] Error: Announcement '{title}' has no Audio URL.")
#       return
#     if path.startswith('/'): path = path[1:]
#     audio_url = f"https://{WINDOWS_IP}:5001/{path}"

#     print(f"\n[!] TRIGGERED: {title}")
#     print(f"[*] Downloading: {audio_url}")

#     r = requests.get(audio_url, verify=False, timeout=10)
#     with open("current_announcement.mp3", "wb") as f:
#         f.write(r.content)

#     subprocess.run(["mpg321", "current_announcement.mp3"])

#     requests.post(f"{BASE_URL}/mark-played/{data.get('id')}", verify=False)

#   except Exception as e:
#     print(f"Error playing audio: {e}")

# print("--- Barangay RPi Multi-Trigger System Active ---")

# while True:
#     try:
#         try:
#           requests.post(f"{BASE_URL}/heartbeat", verify=False, timeout=2)
#         except:
#             print("!", end="", flush=True)
#         emergency = requests.get(f"{BASE_URL}/latest", verify=False, timeout=3)
#         if emergency.status_code == 200:
#             play_audio(emergency.json())
#             continue

#         manual = requests.get(f"{BASE_URL}/manual-trigger", verify=False, timeout=3)
#         if manual.status_code == 200:
#             play_audio(manual.json())
#             continue

#         scheduled = requests.get(f"{BASE_URL}/latest", verify=False, timeout=3)
#         if scheduled.status_code == 200:
#             play_audio(scheduled.json())

#     except Exception as e:
#         print(f"Connection error: {e}")
    
#     print(".", end="", flush=True)
#     time.sleep(5)
