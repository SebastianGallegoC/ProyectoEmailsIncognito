/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        // Chrome Incognito Theme Colors
        incognito: {
          darker: '#121212',
          dark: '#202124',
          medium: '#292A2D',
          light: '#3C4043',
          lighter: '#5F6368',
          text: '#E8EAED',
          subtext: '#9AA0A6',
          accent: '#8AB4F8',
          'accent-hover': '#669DF6',
        },
        primary: {
          50: '#f0f9ff',
          100: '#e0f2fe',
          200: '#bae6fd',
          300: '#7dd3fc',
          400: '#38bdf8',
          500: '#0ea5e9',
          600: '#0284c7',
          700: '#0369a1',
          800: '#075985',
          900: '#0c4a6e',
        },
      },
    },
  },
  plugins: [],
}
