# uninstall
helm uninstall simple-concert-service
helm uninstall simple-cinema-service
helm uninstall ingress-routing
# install
helm install ingress-routing ./ingress-routing
helm install simple-concert-service ./microservice-generic -f simple-concert-service.yaml
helm install simple-cinema-service ./microservice-generic -f simple-cinema-service.yaml
