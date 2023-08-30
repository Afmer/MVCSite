FROM asigatchov/sphinx:2.2.11

COPY ../../sphinx.conf /etc/sphinxsearch/sphinx.conf
RUN mkdir -p ./data
CMD ["/bin/bash", "-c", "indexer --all;searchd --nodetach"]