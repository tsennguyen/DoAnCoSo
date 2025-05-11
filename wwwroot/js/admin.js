$(document).ready(function () {
    // --- Sidebar Toggle Logic ---
    function toggleSidebar() {
        $("body").toggleClass("sidebar-toggled");
        // Optional: Save state to localStorage
        if ($("body").hasClass("sidebar-toggled")) {
            localStorage.setItem('sidebarToggled', 'true');
        } else {
            localStorage.removeItem('sidebarToggled');
        }
    }

    // Check localStorage on page load
    if (localStorage.getItem('sidebarToggled') === 'true') {
        $("body").addClass("sidebar-toggled");
    }

    // Click handler for both toggle buttons
    $("#sidebarToggleTop, #sidebarToggleDesktop").on('click', function (e) {
        e.preventDefault();
        toggleSidebar();
    });

    // Optional: Close sidebar on mobile when clicking outside (requires more setup)

    // Optional: Add smooth scrolling for internal links if needed
    // $('a.nav-link[href*="#"]:not([href="#"])').click(function() { ... });

});

/*order index*/
/* =====================================
   Admin Order List Page Styles
   ===================================== */

.admin - order - list - container.section - title {
    /* K? th?a style */
}

/* --- Order Table --- */
#orderListTable { /* Target b?ng ??n h�ng */
    /* K? th?a style .admin-table */
    font - size: 0.88rem; /* C� th? nh? h?n n?a n?u c?n */
}
#orderListTable thead.table - light th {
    /* K? th?a */
}
#orderListTable tbody td {
    /* K? th?a */
    line - height: 1.4; /* Gi?m line-height n?u mu?n b?ng ch?t h?n */
    padding - top: 0.6rem;
    padding - bottom: 0.6rem;
}

/* --- Column Widths & Specific Styles (?i?u ch?nh n?u c?n) --- */
#orderListTable.stt - col { width: 4 %; }
#orderListTable.ordercode - col { width: 15 %; }
#orderListTable.user - col { width: 18 %; }
#orderListTable.date - col { width: 13 %; white - space: nowrap; }
#orderListTable.total - col { width: 12 %; white - space: nowrap; } /* C?t t?ng ti?n */
#orderListTable.status - col { width: 10 %; }
#orderListTable.payment - col { width: 10 %; }
#orderListTable.action - col { width: 8 %; white - space: nowrap; }

/* Style link trong c?t Order Code */
#orderListTable.ordercode - col a {
    font - weight: 600; /* ??m h?n */
    color: #0056b3; /* M�u xanh */
    text - decoration: none;
}
#orderListTable.ordercode - col a:hover {
    text - decoration: underline;
}

/* Style c?t t?ng ti?n */
#orderListTable.total - col {
    /* font-weight: bold; */ /* ?� c� fw-bold */
    color: #dc3545; /* M�u ?? cho t?ng ti?n (t�y ch?n) */
}

/* Style Badge cho Payment Method */
#orderListTable.payment - col.badge {
    /* K? th?a style .badge chung */
    font - size: 0.75rem;
    padding: 4px 7px;
}
#orderListTable.payment - col.bg - momo - custom {
    /* ?� c� t? style tr??c */
    text - decoration: none; /* ??m b?o link badge kh�ng c� g?ch ch�n */
}
#orderListTable.payment - col.bg - momo - custom:hover {
    opacity: 0.9; /* Hi?u ?ng hover nh? */
}
#orderListTable.payment - col.bg - vnpay - custom {
    /* ?� c� t? style tr??c */
}

/* Style N�t H�nh ??ng */
#orderListTable.action - col.btn - sm {
    /* K? th?a */
    margin: 0 2px;
}
#orderListTable.action - col.btn - info { /* N�t Xem */
    color: #fff;
}
#orderListTable.action - col.btn - warning { /* N�t H?y (n?u c�) */
    color: #fff;
}

/* --- Ph�n trang v� Th�ng b�o (K? th?a) --- */