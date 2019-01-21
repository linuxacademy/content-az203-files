docker image ls
docker build -t webapp .
docker run -d -p 8080:80 --name myapp webapp
docker ps -a
