let dataTable;

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "20%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                       <div class="d-flex justify-content-center"
                        <div class="w-75 btn-group details-tab" role="group">
                            <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-info d-flex justify-content-center align-items-center"><i class="bi bi-pencil-square me-1"></i>Details</a>
                    </div>
                </div>
                    `
                },
                "width": "10%"
            }
        ]
    });
}

$(document).ready(function () {
    loadDataTable();
});