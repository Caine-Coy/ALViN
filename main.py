import asyncio
import requests
import socket
import datetime

from bleak import BleakScanner

#Database URL
POCKETBASE_URL = 'http://192.168.1.168:8080/api'
BEACON_URL = '/collections/beacons/records'
DEVICE_URL = '/collections/devices/records'
DEVICE_NAME = socket.gethostname()
CYCLE_TIME = 10

LAST_UUID = ""

async def update_device(beacon):

    data = {
        "name": DEVICE_NAME,
        "lastuuid": beacon['uuid'],
        "lastdetected": datetime.datetime.utcnow().isoformat() + "Z",  # Current time in ISO 8601 format
        "lastbeacon": beacon['id']
    }

    current_device_url = f"{POCKETBASE_URL}{DEVICE_URL}?filter=name='{DEVICE_NAME}'"
    response = requests.get(current_device_url)
    
    if response.ok and response.json()['items']:
        # Device exists, get the unique ID
        device_id = response.json()['items'][0]['id']
        # Update the existing device record
        update_url = f"{POCKETBASE_URL}{DEVICE_URL}/{device_id}"
        response = requests.patch(update_url, json=data)
    else:
        # Device does not exist, create a new record
        response = requests.post(POCKETBASE_URL + DEVICE_URL, json=data)

    if response.ok:
        print("Updated device info successfully.")
    else:
        print(f"Failed to update device info: {response.content}")

async def printScan():
    devices = await BleakScanner.discover()
    for d in devices:
        print(f"{d.address} called {d.name}")

async def scan_and_update():
    db_beacons = await get_beacons() 
    devices = await BleakScanner.discover()
    beacon_dict = {}
    # Collect RSSI values for each beacon
    for d in devices:
        uuid = str(d.address)  
        for b in db_beacons:
            if uuid in b:  # Check if the scanned device's UUID is in the database
                if uuid not in beacon_dict:
                    beacon_dict[b] = []
                beacon_dict[b].append(d.rssi)
                break

    # Calculate the average RSSI for each beacon
    avg_beacons = {uuid: sum(rssis)/len(rssis) for uuid, rssis in beacon_dict.items()}

    # Select the beacon with the highest average RSSI (least negative)
    closest_beacon_uuid = max(avg_beacons, key=avg_beacons.get, default=None)

    # Select the beacon with the highest average RSSI
    closest_beacon_uuid = max(avg_beacons, key=avg_beacons.get, default=None)
    if closest_beacon_uuid:
        closest_beacon = db_beacons[closest_beacon_uuid]
        print(f"Closest beacon: {closest_beacon['uuid']} with UUID: {closest_beacon_uuid}")
        await update_device(closest_beacon)
    else:
        print(f"No matching beacons detected. {uuid}")

async def get_beacons():
    response = requests.get(POCKETBASE_URL + BEACON_URL)
    if response.ok:
        beacons = response.json().get('items', [])
        return {beacon['uuid']: beacon for beacon in beacons}
    else:
        print(f"Failed to retrieve beacons: {response.content}")
        return []
    
async def get_devices():
    response = requests.get(POCKETBASE_URL + BEACON_URL)
    if response.ok:
        devices = response.json().get('items', [])
        return {devices['uuid']: device for device in devices}
    else:
        print(f"Failed to retrieve devicess: {response.content}")
        return []

async def main():
    while True:
        await scan_and_update()
        await printScan()
        await asyncio.sleep(CYCLE_TIME)


if __name__ == "__main__":
    asyncio.run(main())