# Frontend

React + TypeScript SPA for the ToDo Aspire sample application.

## Overview

A weather forecast dashboard built with **Vite**, **React 19**, and **TypeScript**. Demonstrates API integration with the Aspire backend and accessible component patterns.

## Features

- Weather forecast display from `/api/weatherforecast`
- Temperature unit toggle (Celsius/Fahrenheit)
- Loading states and error handling
- Accessible UI with ARIA labels

## Quick Start

```bash
npm install
npm run dev
```

## Project Structure

```
frontend/
├── src/
│   └── App.tsx          # Main weather component
├── public/              # Static assets
├── index.html           # Entry point
├── vite.config.ts       # Vite configuration
└── tsconfig.json        # TypeScript config
```

## Key Component

```tsx
// App.tsx - Weather data fetching
const [weatherData, setWeatherData] = useState<WeatherForecast[]>([])

useEffect(() => {
  fetch('/api/weatherforecast')
    .then(res => res.json())
    .then(setWeatherData)
}, [])
```

## Scripts

| Command | Description |
|---------|-------------|
| `npm run dev` | Start development server |
| `npm run build` | Build for production |
| `npm run preview` | Preview production build |
| `npm run lint` | Run ESLint |

## Integration

The frontend is configured as part of the Aspire AppHost. The API proxy is set up in `vite.config.ts` to forward `/api/*` requests to the backend service.

## Related Documentation

- **[Deepstaging README](../../../../README.md)** — Main project overview
- **[AppHost](../Deepstaging.ToDo.AppHost/README.md)** — Aspire orchestration
- **[Server](../Deepstaging.ToDo.Server/README.md)** — Backend API
- **[Domain Library](../../Deepstaging.ToDo/README.md)** — Effects-based domain model
