# Deploy to k8s
```
kubectl create namespace blockchain
kubectl -n blockchain deploy -f .
```

## Run IoT Devices:
make sure that the [NODE_IP] field points at rabbitMQ's deployment endpoint within the k8s cluster.
```
docker run -it -e DEVICE_NAME=Sensor_1 -e DEVICE_SLEEP=5000 -e RABBITMQ_CONN="amqp://guest:guest@[NODE_IP]:30002" davidetestoni/iotdevice
```

## Query the IoT devices. Run in UEs container:
```
curl -H "Token: 0x67c75466b21d564bab161e0c214fb0db3bf385ad|0x130c1d7875056b63a3dc30cb298c12dcca0443c0|1625231289|0xb43e39502ee92e42deacd71ca954bb2362f701cbc30799c4c9cf471f66b917cc09e27ea0beb6a6bd392a6353656edfbe60130adb313dd8c32842f8f3597910e61c" --interface oaitun_ue3 -v "http://[NODE_IP]:30000/humidity/latest?count=10&deviceNames=Sensor_1"
```


replace [NODE_IP] with k8s cluster node's IP where iotdataprovider's pod is deployed.

with the above _Token_ the device is authorized to access Sensor_1 but not Sensor_2.
