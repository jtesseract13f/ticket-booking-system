# uninstall
# PostgreSQL
helm uninstall postgresql
helm uninstall kafka
helm uninstall simple-concert-service
helm uninstall simple-cinema-service
helm uninstall identity-provider
helm uninstall concert-aggregator
helm uninstall ingress-routing
helm uninstall gateway
helm uninstall email-service
helm uninstall libretranslate

# install
helm install postgresql bitnami/postgresql -f ./db_values.yaml --wait --timeout 5m
helm install kafka helmforge/kafka --wait --timeout 5m
helm install ingress-routing ./ingress-routing --wait --timeout 5m
helm install simple-concert-service ./microservice-generic -f simple-concert-service.yaml
helm install simple-cinema-service ./microservice-generic -f simple-cinema-service.yaml
helm install identity-provider ./microservice-generic -f identity-provider-values.yaml
helm install concert-aggregator ./microservice-generic -f concert-aggregator-values.yaml
helm install gateway ./microservice-generic -f gateway-values.yaml
helm install email-service ./microservice-generic -f email-service-values.yaml
helm install libretranslate libretranslate/libretranslate -f llm-values.yaml