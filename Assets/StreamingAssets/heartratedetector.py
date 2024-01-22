import requests

api_url = 'https://codeberg.org/Freeyourgadget/Gadgetbridge/wiki/Huami-Server-Pairing'
api_key = '94359d5b8b092e1286a43cfb62ee7923'

headers = {
    'Authorization': f'Bearer {api_key}'
}

try:
    response = requests.get(api_url, headers=headers)

    if response.status_code == 200:
        heart_rate_data = response.json()
        print(f"Heart Rate: {heart_rate_data['heart_rate']}")
    else:
        print(f"Error: {response.status_code} - {response.text}")

except requests.RequestException as e:
    print(f"Request failed: {e}")
