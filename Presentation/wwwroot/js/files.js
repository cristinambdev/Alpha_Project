﻿document.addEventListener('DOMContentLoaded', () => {
    const previewSize = 150

    //open modal
    const modalButtons = document.querySelectorAll('[data-modal="true"]')
    modalButtons.forEach(button => {
        button.addEventListener('click', () => {
            const modalTarget = button.getAttribute('data-target')

            const modal = document.querySelector(modalTarget)
            if (modal)
                modal.style.display = 'flex';

        })
    })



    //close modal
    const closeButtons = document.querySelectorAll('[data-close="true"]')

    closeButtons.forEach(button => {
        button.addEventListener('click', () => {
            const modal = button.closest('.modal')
            if (modal) {
                modal.style.display = 'none'

                //clear formdata
                modal.querySelectorAll('form').forEach(form => {
                    form.reset()


                    const imagePreview = form.querySelector('.image-preview')
                    if (imagePreview)
                        imagePreview.src = ''

                    const imagePreviewer = form.querySelector('.image-previewer')
                    if (imagePreviewer)
                        imagePreviewer.classList.remove('selected')

                })
            }
        })

    })

    // Close modal when clicking outside modal content generated by chatGPT
    document.querySelectorAll('.modal').forEach(modal => {
        modal.addEventListener('click', function (e) {
            if (e.target === modal) {
                modal.style.display = 'none';
            }
        });
    });
    document.addEventListener('DOMContentLoaded', () => {
        function setupDropdown(toggleSelector, menuSelector) {
            const toggleButtons = document.querySelectorAll(toggleSelector);

            toggleButtons.forEach(button => {
                button.addEventListener('click', (event) => {
                    event.stopPropagation(); // Prevent click from bubbling up

                    const isExpanded = button.getAttribute('aria-expanded') === 'true';
                    const dropdownMenu = document.querySelector(button.getAttribute('aria-controls'));

                    // Close all other dropdowns before opening the clicked one
                    document.querySelectorAll(menuSelector).forEach(menu => {
                        if (menu !== dropdownMenu) {
                            menu.classList.add('hide');
                            const otherButton = document.querySelector(`[aria-controls="${menu.id}"]`);
                            if (otherButton) {
                                otherButton.setAttribute('aria-expanded', false);
                            }
                        }
                    });

                    if (isExpanded) {
                        button.setAttribute('aria-expanded', false);
                        dropdownMenu.addEventListener('animationend', () => {
                            dropdownMenu.classList.add('hide');
                        }, { once: true });
                    } else {
                        button.setAttribute('aria-expanded', true);
                        dropdownMenu.classList.remove('hide');
                    }
                });
            });

            // Close dropdown when clicking outside
            document.addEventListener('click', (event) => {
                document.querySelectorAll(menuSelector).forEach(menu => {
                    if (!menu.contains(event.target)) {
                        menu.classList.add('hide');
                        const button = document.querySelector(`[aria-controls="${menu.id}"]`);
                        if (button) {
                            button.setAttribute('aria-expanded', false);
                        }
                    }
                });
            });
        }

        // Initialize dropdowns
        setupDropdown('.fa-ellipsis', '.menu-dropdown-project');
        setupDropdown('.bi-gear-fill', '.settings');
    });
    //// open close project dropdown menu
    //const dropdownMenuBtn = document.querySelector('.fa-ellipsis')
    //const dropdownMenu = document.querySelector('#projectDropdown')


    //dropdownMenuBtn.addEventListener('click', () => {

    //    const isExpanded = dropdownMenuBtn.getAttribute('aria-expanded') === 'true'


    //    if (isExpanded) {

    //        dropdownMenuBtn.setAttribute('aria-expanded', false)
    //        dropdownMenu.addEventListener('animationend', () => {
    //            dropdownMenu.classList.add('hide')

    //        }, { once: true })

    //    }
    //    else {
    //        dropdownMenuBtn.setAttribute('aria-expanded', true)
    //        dropdownMenu.classList.remove('hide')

    //    }
    //})



    // handle image-previewer
    document.querySelectorAll('.image-previewer').forEach(previewer => {
        const fileInput = previewer.querySelector('input[type="file"]')
        const imagePreview = previewer.querySelector('.image-preview')


        previewer.addEventListener('click', () => fileInput.click())

        fileInput.addEventListener('change', ({ target: { files } }) => {
            const file = files[0]
            if (file)
                processImage(file, imagePreview, previewer, previewSize)

        })
    })


    //handle submit forms

    const forms = document.querySelectorAll('form')
    forms.forEach(form => {
        form.addEventListener('submit', async (e) => {
            e.preventDefault() // stops site to uppdate


            clearErrorMessages(form)  // Clear previous error messages

            const formData = new FormData(form)


            try {
                const res = await fetch(form.action, {
                    method: 'post',
                    body: formData
                })

                if (res.ok) {
                    const modal = form.closest('.modal')
                    if (modal)
                        modal.style.display = 'none';

                    window.location.reload()
                }
                else if (res.status === 400) {
                    const data = await res.json()

                    if (data.errors) {
                        Object.keys(data.errors).forEach(key => {

                            let input = form.querySelector(`[name="${key}"]`)
                            if (input) {
                                input.classList.add('input-validation-error')
                            }


                            let span = form.querySelector(`[data-valmsg-for="${key}"]`)
                            if (span) {
                                span.innerText = data.errors[key].join('\n')
                                span.classList.add('field-validation-error')
                            }
                        })
                    }
                }
            }
            catch {
                console.log('error submitting the form')
            }

        })

    })
})

function clearErrorMessages(form) {

    form.querySelectorAll('[data-val="true"]').forEach(input => {
        input.classList.remove('input-validation-error')
    })
    form.querySelectorAll('[data-valmsg-for]').forEach(span => {
        span.innerText = ''
        span.classList.remove('field-validation-error')
    })
}


async function loadImage(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader()

        reader.onerror = () => reject(new Error("Failed to load file."))
        reader.onload = (e) => {
            const img = new Image()
            img.onerror = () => reject(new Error("Failed to load image."))
            img.onload = () => resolve(img)
            img.src = e.target.result
        }
        reader.readAsDataURL(file)
    })
}

async function processImage(file, imagePreview, previewer, previewSize = 150) {
    try {
        const img = await loadImage(file)
        const canvas = document.createElement('canvas')
        canvas.width = previewSize
        canvas.height = previewSize

        const ctx = canvas.getContext('2d')
        ctx.drawImage(img, 0, 0, previewSize, previewSize)

        imagePreview.src = canvas.toDataURL('image/jpeg')
        previewer.classList.add('selected')
    }
    catch (error) {
        console.error("Failed on image-processing: ", error)
    }
}
