FROM ubuntu:18.04
RUN apt-get update && apt-get install -y ca-certificates

ADD . .

CMD ["/LinuxServer/dedicatedServer.x86_64", "-nographics", "-batchmode"]