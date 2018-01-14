import socket

port = 6971
sck
tcp_ip
tcp_count = 128

def InitCom(_ip):
    tcp_ip = _ip
    sck = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
    sck.connect(tcp_ip,port)

def CloseCom():
    sck.close()

def Receive():
    data = sck.recv(tcp_count)

def Send(data):
    sck.sendall(data)