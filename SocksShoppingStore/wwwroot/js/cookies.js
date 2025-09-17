(() => {
  const banner = document.getElementById('cookie-consent');
  if (!banner) return;

  const consentCookie = 'cookie-consent=accepted';
  const hasConsent = document.cookie.split(';').some(c => c.trim().startsWith('cookie-consent='));
  if (!hasConsent) {
    banner.classList.add('is-visible');
  }

  const acceptBtn = document.getElementById('cookie-accept');
  if (acceptBtn) {
    acceptBtn.addEventListener('click', () => {
      const oneYear = 365 * 24 * 60 * 60;
      document.cookie = consentCookie + `; Max-Age=${oneYear}; Path=/; SameSite=Lax`;
      banner.classList.remove('is-visible');
    });
  }
})();
