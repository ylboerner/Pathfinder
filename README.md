# Pathfinder

See it live in action: https://ylboerner.github.io/Pathfinder/

## URL parameters

Here are some examples for prefilling the input via url parameters:

`fhirPath`

```
https://ylboerner.github.io/Pathfinder/?fhirPath=Patient.name.given%5B0%5D%20%3D%20%27Peter%27
```

`instance`
```
https://ylboerner.github.io/Pathfinder/?instance=%7B%0D%0A++%22resourceType%22%3A+%22Patient%22%2C%0D%0A++%22id%22%3A+%22example%22%0D%0A%7D
````

Both parameters in conjunction: 

```
https://ylboerner.github.io/Pathfinder/?fhirPath=Patient.name.given%5B0%5D%20%3D%20%27Peter%27?instance=%7B%0D%0A++%22resourceType%22%3A+%22Patient%22%2C%0D%0A++%22id%22%3A+%22example%22%0D%0A%7D
```