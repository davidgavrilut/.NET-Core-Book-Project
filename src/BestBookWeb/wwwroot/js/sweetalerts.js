function validateInput() {
    if (!document.getElementById("uploadBox").value) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please an image!'
        });
        return false;
    }
    return true;
}