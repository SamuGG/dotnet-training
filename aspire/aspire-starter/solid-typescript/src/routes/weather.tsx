import { Suspense, For } from "solid-js"
import { query, createAsync } from "@solidjs/router"

interface WeatherData {
  date: string
  temperatureC: number
  temperatureF: number
  summary: string
}

const getForecasts = query(async () => {
  'use server'
  const url = `${process.env.APISERVICE_HTTP}/weatherforecast`

  try {
    const response = await fetch(url)
    if (!response.ok)
      throw new Error(`Response status: ${response.status}`)

    const result = await response.json() as WeatherData[]
    // console.log(result)
    return result
  } catch (error) {
    console.error(error)
  }
}, "weather-forecasts")

export default function Page() {
  const forecasts = createAsync(() => getForecasts())
  return (
    <table style="margin: 8px; pading: 8px;">
      <thead>
        <tr>
          <th>Date</th>
          <th>TemperatureC</th>
          <th>Summary</th>
        </tr>
      </thead>
      <tbody>
        <Suspense fallback={<tr><td colspan="3">Loading...</td></tr>}>
          <For each={forecasts()}>{(forecast) =>
            <tr>
              <td>{forecast.date}</td>
              <td style="text-align: right">{forecast.temperatureC}</td>
              <td>{forecast.summary}</td>
            </tr>
          }</For>
        </Suspense>
      </tbody>
    </table>
  );
}