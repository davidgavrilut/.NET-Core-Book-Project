let dataTable;

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "city", "width": "15%" },
            { "data": "state", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="d-flex">
                        <div class="w-75 btn-group" role="group">
                            <a href="/Admin/Company/Upsert?id=${data}" class="btn btn-outline-info mx-2 d-flex justify-content-center align-items-center"><i class="bi bi-pencil-square me-1"></i>Edit</a>
                    </div>
                        <div class="w-75 btn-group" role="group">
                            <a onClick="deleteCompany('/Admin/Company/Delete/${data}')" class="btn btn-outline-danger mx-2 d-flex justify-content-center align-items-center"><i class="bi bi-trash-fill me-1"></i>Delete</a>
                        </div>
                        </div>
                    `
                },
                "width": "15%"
            }
        ]
    });
}

function deleteCompany(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            let options = {};
            options.method = 'DELETE';
            fetch(url, options)
                .then((res) => res.json())
                .then((data) => {
                    if (data.success) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                    } else {
                    toastr.error(data.message);
                }
            })
        }
    })
}   

$(document).ready(function () {
    loadDataTable();
});