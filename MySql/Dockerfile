FROM mysql:5.7
ENV MYSQL_ROOT_PASSWORD=testdbpass
ENV MYSQL_DATABASE=salto
ENV MYSQL_USER=saltouser
ENV MYSQL_PASSWORD=saltopass

COPY ./init/ /docker-entrypoint-initdb.d/

CMD ["mysqld", "--character-set-server=utf8mb4", "--collation-server=utf8mb4_unicode_ci"]
