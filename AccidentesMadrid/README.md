El programa consiste de un modelo Accidente para utilizar datos de los CSV. Para ello se hará uso del mapper, que mapea directamente del CSV al modelo Accidente.  

El repositorio solo tiene la función de traer todos los datos de los CSV a una lista mapeándolos en el proceso.  

En las consultas de LINQ se hace uso de PLINQ cuando su overhead no resultará en una pérdida de rendimiento. Se ha estimado debido a la cantidad de registros que se manejan que todas las consultas con operaciones más complejas como GroupBy deberán usar PLINQ ya que reportará mejor
rendimiento, en cambio en consultas más simple no, ya que resultará en lo contrario.  

En las consultas con DataFrames aunque en teoría deberían reportar el mayor rendimiento, en la mayoría se ha optado por una opción híbrida entre LINQ y DataFrames debido a que la complejidad de las consultas dificultaba el uso de estos. Está opción híbrida reporta un peor rendimiento
que solo LINQ, en cambio las consultas con DataFrames puros darán el mejor rendimiento.  

De este proyecto se puede sacar en claro que los DataFrames son los más eficientes con cantidades de datos enormes pero dificultan la creación de consultas complejas, en cambio LINQ las facilita a cambio de un peor rendimiento con estas cantidades de datos, pero gracias a PLINQ
puede mejorar este para acercarlo al de los DataFrames, aunque sin lograr llegar a su nivel.
