// https://vite.dev/config/
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/gateway': {
        target: 'http://ticket-booking-system.local',
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
