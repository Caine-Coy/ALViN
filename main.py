import asyncio
import requests
import socket
from bleak import BleakScanner

#Database URL
POCKETBASE_URL = 'http://192.168.1.168:8080/api'
DEVICE_NAME = socket.gethostname()

async def update_device():
    
    data = {
        "name": DEVICE_NAME,
        "beacon": beacon
    }

    response = requests.post(POCKETBASE_URL, json=data)
    if response.ok:
        print("Updated device info successfully.")
    else:
        print(f"Failed to update device info: {response.content}")

async def scan_and_update():
    devices = await BleakScanner.discover()
    localBeacons = [{'name': d.name, 'rssi': d.rssi} for d in devices]
    print(localBeacons)
     #await update_device(beacons)

async def main():
    while True:
        await scan_and_update()
        await asyncio.sleep(60)  # Delay for 60 seconds

if __name__ == "__main__":
    asyncio.run(main())