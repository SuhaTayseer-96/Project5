var simplemaps_countrymap_mapdata={
  main_settings: {
   //General settings
    width: "responsive", //'700' or 'responsive'
    background_color: "#FFFFFF",
    background_transparent: "yes",
    border_color: "#ffffff",
    
    //State defaults
    state_description: "State description",
    state_color: "#b17c07",
    state_hover_color: "#b8974d",
    state_url: "",
    border_size: 1.5,
    all_states_inactive: "no",
    all_states_zoomable: "yes",
    
    //Location defaults
    location_description: "Location description",
    location_url: "",
    location_color: "#FF0067",
    location_opacity: 0.8,
    location_hover_opacity: 1,
    location_size: "25",
    location_type: "square",
    location_image_source: "frog.png",
    location_border_color: "#FFFFFF",
    location_border: 2,
    location_hover_border: 2.5,
    all_locations_inactive: "no",
    all_locations_hidden: "no",
    
    //Label defaults
    label_color: "#ffffff",
    label_hover_color: "#ffffff",
    label_size: 16,
    label_font: "Arial",
    label_display: "auto",
    label_scale: "yes",
    hide_labels: "no",
    hide_eastern_labels: "no",
   
    //Zoom settings
    zoom: "yes",
    manual_zoom: "yes",
    back_image: "no",
    initial_back: "no",
    initial_zoom: "-1",
    initial_zoom_solo: "no",
    region_opacity: 1,
    region_hover_opacity: 0.6,
    zoom_out_incrementally: "yes",
    zoom_percentage: 0.99,
    zoom_time: 0.5,
    
    //Popup settings
    popup_color: "white",
    popup_opacity: 0.9,
    popup_shadow: 1,
    popup_corners: 5,
    popup_font: "12px/1.5 Verdana, Arial, Helvetica, sans-serif",
    popup_nocss: "no",
    
    //Advanced settings
    div: "map",
    auto_load: "yes",
    url_new_tab: "no",
    images_directory: "default",
    fade_time: 0.1,
    link_text: "View Website",
    popups: "detect",
    state_image_url: "",
    state_image_position: "",
    location_image_url: ""
  },
  state_specific: {
    JOAJ: {
      name: "Ajloun",
      hover_color: "#5a8900",
      zoomable: "yes"
    },
    JOAM: {
      name: "Amman",
      description: " ",
      zoomable: "no"
    },
    JOAQ: {
      name: "Aqaba",
      description: " ",
      zoomable: "no"
    },
    JOAT: {
      name: "Tafilah",
      description: " ",
      zoomable: "no"
    },
    JOAZ: {
      name: "Zarqa",
      description: " ",
      zoomable: "no"
    },
    JOBA: {
      name: "Balqa",
      description: " ",
      zoomable: "no"
    },
    JOIR: {
      name: "Irbid",
      description: " ",
      hover_color: "#1d77c0",
      zoomable: "yes"
    },
    JOJA: {
      name: "Jarash",
      description: " ",
      zoomable: "no"
    },
    JOKA: {
      name: "Karak",
      description: " ",
      zoomable: "no"
    },
    JOMA: {
      name: "Mafraq",
      description: " ",
      zoomable: "no"
    },
    JOMD: {
      name: "Madaba",
      description: " ",
      zoomable: "no"
    },
    JOMN: {
      name: "Ma`an",
      description: " ",
      zoomable: "no"
    }
  },
  locations: {},
  labels: {
    "1": {
      name: "Irbid",
      parent_id: "JOIR"
    },
    "2": {
      name: "Irbid",
      parent_id: "JOIR"
    },
    "3": {
      name: "اربد الاولى",
      x: 245.89226,
      y: "226.3771",
      size: "40",
      parent_id: "JOIR"
    },
    "4": {
      name: "اربد الثانية",
      x: 255.49495,
      y: 200.90909,
      parent_id: "JOIR",
      size: "40"
    },
    JOAJ: {
      name: "Ajlun",
      parent_id: "JOAJ"
    },
    JOAM: {
      name: "Amman",
      parent_id: "JOAM"
    },
    JOAQ: {
      name: "Aqaba",
      parent_id: "JOAQ"
    },
    JOAT: {
      name: "Tafilah",
      parent_id: "JOAT"
    },
    JOAZ: {
      name: "Zarqa",
      parent_id: "JOAZ"
    },
    JOBA: {
      name: "Balqa",
      parent_id: "JOBA"
    },
    JOIR: {
      name: "Irbid",
      parent_id: "JOIR"
    },
    JOJA: {
      name: "Jarash",
      parent_id: "JOJA"
    },
    JOKA: {
      name: "Karak",
      parent_id: "JOKA"
    },
    JOMA: {
      name: "Mafraq",
      parent_id: "JOMA"
    },
    JOMD: {
      name: "Madaba",
      parent_id: "JOMD"
    },
    JOMN: {
      name: "Ma`an",
      parent_id: "JOMN"
    }
  },
  legend: {
    entries: []
  },
  regions: {},
  data: {
    data: {}
  }
};