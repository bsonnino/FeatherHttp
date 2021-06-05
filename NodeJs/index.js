const express = require("express");

const app = express();

const nextElement = (n) => Math.floor(Math.random() * n);

const summaries = [
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
];

app.get("/weatherforecast", (req, res, next) => {

    const forecasts = [...Array(5).keys()].map(i => (
        {
            "date": new Date() + i,
            "temperatureC": nextElement(75) - 20,
            "summary": summaries[nextElement(summaries.length)]
        }));
    
    res.json(forecasts);
});

app.listen(3000, () => {
    console.log("Server running on port 3000");
});