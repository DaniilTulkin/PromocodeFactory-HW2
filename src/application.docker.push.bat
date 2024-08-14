docker login ghcr.io --username DaniilTulkin --password 
docker build . -t ghcr.io/daniiltulkin/promo-code-factory:latest 
docker push ghcr.io/daniiltulkin/promo-code-factory:latest

pause