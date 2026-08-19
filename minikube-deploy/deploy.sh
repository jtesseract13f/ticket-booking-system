# uninstall
# PostgreSQL
helm uninstall postgresql
helm uninstall kafka
helm uninstall simple-concert-service
helm uninstall simple-cinema-service
helm uninstall ingress-routing
# install

helm install postgresql bitnami/postgresql -f ./db_values.yaml --wait --timeout 5m
helm install kafka helmforge/kafka
helm install ingress-routing ./ingress-routing
helm install simple-concert-service ./microservice-generic -f simple-concert-service.yaml
helm install simple-cinema-service ./microservice-generic -f simple-cinema-service.yaml