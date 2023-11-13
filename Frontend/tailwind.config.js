/** @type {import('tailwindcss').Config} */
export default {
  content: ['./src/**/*.{html,js,svelte,ts}'],
  theme: {
    extend: {
      container: {
        center: true,
        padding: '1rem',
        screens: {
          sm: '320px',
          md: '384px',
          lg: '512px',
          xl: '640px',
          '2xl': '768px',
        },
      },
    },
  },
  plugins: [],
}

