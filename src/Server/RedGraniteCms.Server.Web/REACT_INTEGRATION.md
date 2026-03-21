# React Admin Integration Guide

The React admin application is hosted at `/admin` within the MVC Server.Web project.

## How It Works

1. **Development Mode**: 
   - Run the React app separately: `npm run dev` in `Client/RedGraniteCms.Client.Web`
   - Access it at `http://localhost:3000`
   
2. **Production Mode**: 
   - The React app is automatically built during Release builds of the Server.Web project
   - Built files are copied to `wwwroot/admin`
   - Access it at `https://your-domain/admin`

## Manual Build & Deploy

If you want to manually build and deploy the React app:

1. Navigate to the React project:
   ```bash
   cd Client/RedGraniteCms.Client.Web
   ```

2. Build the production version:
   ```bash
   npm install
   npm run build
   ```

3. Copy the `build` folder contents to `Server/RedGraniteCms.Server.Web/wwwroot/admin`

## Configuration

- **Vite Base Path**: The Vite config sets `base: '/admin/'` for production builds
- **Static Files**: Program.cs serves static files from `wwwroot/admin` at the `/admin` path
- **SPA Fallback**: All `/admin/*` routes fall back to `index.html` for client-side routing

## Development Tips

- For development, run both servers:
  - React dev server: `npm run dev` (port 3000)
  - ASP.NET server: `dotnet run` (typically port 5034/5035)
  
- The React app's API calls should point to the ASP.NET GraphQL endpoint
- Check `.env.development` and `.env.production` files for environment-specific configs
