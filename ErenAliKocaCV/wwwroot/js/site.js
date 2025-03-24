// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function() {
    // Base title for the website
    const baseTitle = 'Eren Ali Koca | Software Developer';
    
    // Debounce function to limit the rate at which updatePageTitle function executes
    function debounce(func, wait) {
        let timeout;
        return function() {
            const context = this;
            const args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(context, args), wait);
        };
    }
    
    // Function to update the page title based on visible section
    function updatePageTitle() {
        const sections = document.querySelectorAll('section[id]');
        const pageTitles = document.querySelectorAll('.page-title');
        const scrollPosition = window.scrollY + 200; // Add offset to detect section earlier
        
        let activeSectionFound = false;
        
        // First try to find by section IDs
        for (let i = 0; i < sections.length; i++) {
            const section = sections[i];
            const top = section.offsetTop;
            const height = section.offsetHeight;
            
            if (scrollPosition >= top && scrollPosition <= top + height) {
                // Find the page title in this section
                const pageTitle = section.querySelector('.page-title');
                if (pageTitle) {
                    document.title = `${pageTitle.textContent.trim()} | Eren Ali Koca`;
                    activeSectionFound = true;
                    break;
                }
            }
        }
        
        // If no section was found active, default to the base title
        if (!activeSectionFound) {
            document.title = baseTitle;
        }
    }
    
    // Lazy load images
    function lazyLoadImages() {
        const lazyImages = document.querySelectorAll('[data-src]');
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver((entries, observer) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        img.src = img.dataset.src;
                        img.removeAttribute('data-src');
                        imageObserver.unobserve(img);
                    }
                });
            });
            lazyImages.forEach(img => imageObserver.observe(img));
        } else {
            // Fallback for browsers without IntersectionObserver support
            lazyImages.forEach(img => {
                img.src = img.dataset.src;
                img.removeAttribute('data-src');
            });
        }
    }
    
    // Listen for scroll events with debounce to improve performance
    window.addEventListener('scroll', debounce(updatePageTitle, 200));
    
    // Initial update
    setTimeout(updatePageTitle, 100);
    
    // Initialize lazy loading
    if (document.readyState === 'complete') {
        lazyLoadImages();
    } else {
        window.addEventListener('load', lazyLoadImages);
    }
    
    // Add smooth scrolling to all internal links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            if (targetId === '#') return;
            
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});
