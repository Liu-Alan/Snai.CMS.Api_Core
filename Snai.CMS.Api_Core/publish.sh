cd /data/www/snai.cms.api_core;     
docker stop Snai.CMS.Api_Core; 
docker rm Snai.CMS.Api_Core;     
docker rmi Snai.CMS.Api_Core-Service:v1.0;     
docker build -t Snai.CMS.Api_Core-Service:v1.0 . ;    
docker run -d -p 8030:80 --restart always --name Snai.CMS.Api_Core \
 -v /data/www/snai.cms.api_core/storage:/app/storage \
 -v /data/www/snai.cms.api_core/appsettings.json:/app/appsettings.json \
Snai.CMS.Api_Core-Service:v1.0
