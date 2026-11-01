FROM postgres:16-alpine AS builder

ARG PG_CRON_VERSION=1.6.7

RUN apk add --no-cache build-base curl \
    && curl -fsSL "https://github.com/citusdata/pg_cron/archive/refs/tags/v${PG_CRON_VERSION}.tar.gz" -o /tmp/pg_cron.tar.gz \
    && mkdir /tmp/pg_cron \
    && tar -xzf /tmp/pg_cron.tar.gz -C /tmp/pg_cron --strip-components=1 \
    && make -C /tmp/pg_cron with_llvm=no \
    && make -C /tmp/pg_cron with_llvm=no install

FROM postgres:16-alpine

COPY --from=builder /usr/local/lib/postgresql/pg_cron.so /usr/local/lib/postgresql/pg_cron.so
COPY --from=builder /usr/local/share/postgresql/extension/pg_cron* /usr/local/share/postgresql/extension/
