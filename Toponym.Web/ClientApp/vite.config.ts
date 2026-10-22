import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { resolve } from 'node:path';

export default defineConfig({
  plugins: [react()],
  base: '/assets/bundle/',
  build: {
    outDir: resolve(import.meta.dirname, '../wwwroot/assets/bundle'),
    emptyOutDir: true,
    manifest: 'manifest.json',
    assetsDir: '',
    rollupOptions: {
      input: resolve(import.meta.dirname, 'src/main.tsx'),
    },
  },
  server: {
    port: 5173,
    strictPort: true,
    origin: 'http://localhost:5173',
    cors: { origin: ['http://localhost:5000', 'https://localhost:5001'] },
  },
});
