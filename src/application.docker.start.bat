docker build -f Dockerfile -t promo-code-factory .
docker run -it --rm -p 5001:5001 promo-code-factory

pause