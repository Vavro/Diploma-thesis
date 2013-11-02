Diploma-thesis
==============

Topic: Querying Preprocessed Linked Data Resources

The major goal of the thesis is to create a web service for querying Linked Data resource. In comparison to generic SPARQL endpoints, the service will allow for optimized querying for details of resources and for searching of resources by their properties. In other words, it won't be possible to get answers to generic SPARQL queries. Only a predefined queries (detail, search) will be allowed. The service will be able to gather data from a given SPARQL endpoint, transform them to an form of JSON documents, store the documents to a document store and answer queries for details of resources and search them by predefined properties. The hypothesis is that those kinds of queries can be evaluated in the document store with a better performance than in a generic triple store. This hypothesis will be proved or disproved by the thesis.
