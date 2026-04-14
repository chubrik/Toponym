import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { App } from './App';
import { readInitialState } from './initialState';
import { setLanguage } from './i18n/lang';
import './styles/app.scss';

const initial = readInitialState();
setLanguage(initial.language);

const container = document.getElementById('app-root');
if (!container) throw new Error('#app-root not found');
createRoot(container).render(
  <StrictMode>
    <App initial={initial} />
  </StrictMode>,
);

document.body.style.visibility = 'visible';
