$(document).ready(function () {
    $('#appointmentDate').change(function () {
        var date = $(this).val();
        var barberShopId = $('#BarberShopId').val();

        $.get('/Appointment/GetWorkingHours', { date: date, barberShopId: barberShopId }, function (data) {
            var timeSlots = $('#timeSlots');
            timeSlots.empty();

            data.forEach(function (time) {
                timeSlots.append('<option value="' + time + '">' + time + '</option>');
            });
        });
    });

    $('.service-checkbox').change(function () {
        var total = 0;
        $('.service-checkbox:checked').each(function () {
            total += parseInt($(this).data('price'));
        });
        $('#TotalPrice').text(total);
    });
});

document.addEventListener('DOMContentLoaded', function () {
    const serviceCheckboxes = document.querySelectorAll('.service-checkbox');
    const totalPriceElement = document.getElementById('total-price');

    serviceCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', updateTotalPrice);
    });

    function updateTotalPrice() {
        let totalPrice = 0;
        serviceCheckboxes.forEach(checkbox => {
            if (checkbox.checked) {
                totalPrice += parseFloat(checkbox.dataset.price);
            }
        });
        totalPriceElement.textContent = totalPrice.toFixed(2);
    }
});

function initializeDateAndTimePicker(workingHours) {
    var today = new Date().toISOString().split('T')[0];
    $('#AppointmentDate').attr('min', today);

    $('#AppointmentDate').on('change', function () {
        var selectedDate = new Date($(this).val());
        var dayOfWeek = selectedDate.getDay();
        var workingHour = workingHours.find(wh => wh.Day === dayOfWeek);

        $('#AppointmentTime').empty();
        if (workingHour) {
            for (var time = new Date('1970-01-01T' + workingHour.OpenTime); time < new Date('1970-01-01T' + workingHour.CloseTime); time.setHours(time.getHours() + 1)) {
                var timeString = time.toTimeString().split(' ')[0].substring(0, 5);
                $('#AppointmentTime').append(new Option(timeString, timeString));
            }
        }
    });
}
