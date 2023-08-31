FROM asigatchov/sphinx:2.2.11

COPY sphinx.conf /etc/sphinxsearch/sphinx.conf
RUN mkdir -p ./data
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
RUN echo "* * * * * indexer --rotate RecipesIndex" | crontab -
CMD ["/entrypoint.sh"]