deploy to k8s:

run in UEs container:
curl -H "Token: 0x254ccedc328705d258661c3d1cb852a4f43763e5|0x130c1d7875056b63a3dc30cb298c12dcca0443c0|1625141904|0x3c814411520ea7c879f03009274823f6894710f481ae10a6a99594348aeb407f4bb7b6670df8952f928623d277b412e729c1a99d55d629b249cfb43547e214621c" --interface oaitun_ue3 -v "http://[NODE_IP]:30000/humidity/latest?count=10&deviceNames=Sensor_1"

replace [NODE_IP] with k8s cluster node

with the above token the device is authorized to access Sensor_1 but not Sensor_2.
