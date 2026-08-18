document.addEventListener("DOMContentLoaded", () => {
  const currentPath = window.location.pathname.toLowerCase();
  document.querySelectorAll(".navbar a.nav-link").forEach(link => {
    const href = new URL(link.href, window.location.origin).pathname.toLowerCase();
    if ((href === "/" && currentPath === "/") || (href !== "/" && currentPath.startsWith(href))) {
      link.classList.add("is-active");
      link.setAttribute("aria-current", "page");
    }
  });

  const items = document.querySelectorAll(".book-card, .category-tile, .metric-card, .service-strip > div");
  if (!("IntersectionObserver" in window) || window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
    items.forEach(item => item.classList.add("is-visible"));
    return;
  }

  const observer = new IntersectionObserver(entries => {
    entries.forEach(entry => {
      if (!entry.isIntersecting) return;
      entry.target.classList.add("is-visible");
      observer.unobserve(entry.target);
    });
  }, { threshold: .12 });

  items.forEach((item, index) => {
    item.classList.add("reveal-item");
    item.style.setProperty("--reveal-delay", `${Math.min(index % 4, 3) * 70}ms`);
    observer.observe(item);
  });
});
