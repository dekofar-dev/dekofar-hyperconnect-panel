import { ChangeDetectorRef, Component, HostBinding, OnInit } from '@angular/core';
import { Order } from 'src/app/modules/orders/model/shopfy/order.model';
import { OrdersService } from 'src/app/modules/orders/services/orders.service';

@Component({
  selector: 'app-search-result-inner',
  templateUrl: './search-result-inner.component.html',
})
export class SearchResultInnerComponent implements OnInit {
  @HostBinding('class') class = 'menu menu-sub menu-sub-dropdown p-7 w-325px w-md-375px';
  @HostBinding('attr.data-kt-menu') dataKtMenu = 'true';
  @HostBinding('attr.data-kt-search-element') dataKtSearch = 'content';

  resultModels: Array<ResultModel> = resultModels;
  recentlySearchedModels: Array<ResultModel> = recentlySearchedModels;

  keyword: string = '';
  searching: boolean = false;

  // 🔍 Shopify sipariş sonuçları
  shopifyOrders: Order[] = [];

  constructor(
    private cdr: ChangeDetectorRef,
    private ordersService: OrdersService
  ) {}

  ngOnInit(): void {}

  search(keyword: string) {
    this.keyword = keyword;
    this.searching = true;
    this.shopifyOrders = [];

    if (!keyword || keyword.length < 2) {
      this.searching = false;
      return;
    }

    this.ordersService.searchOrders(keyword).subscribe({
      next: (res) => {
        this.shopifyOrders = res;
        this.searching = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Shopify arama hatası', err);
        this.searching = false;
      },
    });
  }

  clearSearch() {
    this.keyword = '';
    this.shopifyOrders = [];
  }
}

interface ResultModel {
  icon?: string;
  image?: string;
  title: string;
  description: string;
}

const resultModels: Array<ResultModel> = [
  {
    image: './assets/media/avatars/300-6.jpg',
    title: 'Karina Clark',
    description: 'Marketing Manager',
  },
  {
    image: './assets/media/avatars/300-2.jpg',
    title: 'Olivia Bold',
    description: 'Software Engineer',
  },
  {
    image: './assets/media/avatars/300-9.jpg',
    title: 'Ana Clark',
    description: 'UI/UX Designer',
  },
  {
    image: './assets/media/avatars/300-14.jpg',
    title: 'Nick Pitola',
    description: 'Art Director',
  },
  {
    image: './assets/media/avatars/300-11.jpg',
    title: 'Edward Kulnic',
    description: 'System Administrator',
  },
];

const recentlySearchedModels: Array<ResultModel> = [
  {
    icon: './assets/media/icons/duotune/electronics/elc004.svg',
    title: 'BoomApp by Keenthemes',
    description: '#45789',
  },
  {
    icon: './assets/media/icons/duotune/graphs/gra001.svg',
    title: '"Kept API Project Meeting',
    description: '#84050',
  },
  {
    icon: './assets/media/icons/duotune/graphs/gra006.svg',
    title: '"KPI Monitoring App Launch',
    description: '#84250',
  },
  {
    icon: './assets/media/icons/duotune/graphs/gra002.svg',
    title: 'Project Reference FAQ',
    description: '#67945',
  },
  {
    icon: './assets/media/icons/duotune/communication/com010.svg',
    title: '"FitPro App Development',
    description: '#84250',
  },
  {
    icon: './assets/media/icons/duotune/finance/fin001.svg',
    title: 'Shopix Mobile App',
    description: '#45690',
  },
  {
    icon: './assets/media/icons/duotune/graphs/gra002.svg',
    title: '"Landing UI Design" Launch',
    description: '#24005',
  },
];
