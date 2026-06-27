interface WeatherData {
    date: string
    temperatureC: number
    temperatureF: number
    summary: string
}

export function setupWeatherForecast(buttonElement: HTMLButtonElement, containerElement: HTMLDivElement) {
    buttonElement.addEventListener('click', async () => {
        const forecast = await fetchWeather()
        const table = document.createElement("table")

        const row1 = table.insertRow()
        row1.insertCell(0).textContent = "Date"
        row1.insertCell(1).textContent = "℃"
        row1.insertCell(2).textContent = "Summary"

        forecast.forEach(item => {
            const row = table.insertRow();
            row.insertCell(0).textContent = item.date
            row.insertCell(1).textContent = item.temperatureC.toString()
            row.insertCell(2).textContent = item.summary
        });

        containerElement.replaceChildren(table)
    })

    async function fetchWeather(): Promise<WeatherData[]> {
        const response = await fetch('/api/weatherforecast')
        return response.json()
    }
}