import React, { Component, Fragment } from 'react';

import getResource from '../../../resourceHelper';
import { NominationStatuses } from '../../../constants';
import { getNominationClassName, noDataIndication } from '../../../common-functions';
import StickyBlock from '../../../shared/components/StickyBlock';

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    handleShowNominatorComment = (nomination, performanceRecordId) => {
        this.props.onShowNominatorComment(nomination, performanceRecordId);
    }

    handleChangeStatus = (nominationId, status) => {
        this.props.onUpdateStatus(nominationId, status.value);
    }

    renderData = data => {
        return (
            <Fragment key={data.nominationPeriodId}>
                <tr className="group-row">
                    <td className="title" colSpan={6}>{data.title}{data.isInProgress && ` - ${getResource('InProgress')}`}</td>
                </tr>
                {
                    data.nominations.length > 0
                        ? data.nominations.map(this.renderNomination)
                        : <tr><td className="text-center" colSpan={6}>{noDataIndication()}</td></tr>
                }
            </Fragment>
        );
    }

    renderNomination = (nomination, index) => {
        return (
            <tr key={nomination.nominationId}>
                <td className="text-center">{index + 1}</td>
                <td>{`${nomination.nominee.nickName} (${nomination.nominee.fullName})`}</td>
                <td>{nomination.nominee.groupName}</td>
                <td className="text-center">
                    <a onClick={this.handleShowNominatorComment.bind(this, nomination, nomination.nominators[0].performanceRecordId)}>
                        {nomination.nominators.length}
                    </a>
                </td>
                <td>
                    {nomination.nominators.map((n, i, arr) =>
                        <Fragment key={n.performanceRecordId}>
                            <a onClick={this.handleShowNominatorComment.bind(this, nomination, n.performanceRecordId)}>
                                {`${n.creator.nickName} (${n.creator.fullName})`}
                            </a>
                            {i < arr.length - 1 && ', '}
                        </Fragment>)
                    }
                </td>
                <td className="text-center">
                    {<span className={getNominationClassName(nomination.statusId)}>
                        {NominationStatuses.find(x => x.value === nomination.statusId).text}
                    </span>}
                </td>
            </tr>
        );
    }

    render() {
        let content = '';

        if (this.props.tableData) {
            content = this.props.tableData.length > 0 ? this.props.tableData.map(this.renderData)
                : <tr><td className="text-center" colSpan={6}>{noDataIndication()}</td></tr>;
        }

        return (
            <Fragment>
                <StickyBlock threshold={236}>
                    <table className="table table-bordered table-striped tbl-nomination-report">
                        <thead>
                            <tr>
                                <th className="order">{getResource('SymbolCount')}</th>
                                <th className="nominee">{getResource('Nominee')}</th>
                                <th className="team">{getResource('Team')}</th>
                                <th className="vote">{getResource('Vote')}</th>
                                <th className="nominator">{getResource('Nominator')}</th>
                                <th className="status">{getResource('Status')}</th>
                            </tr>
                        </thead>
                    </table>
                </StickyBlock>
                <table className="table table-bordered table-striped tbl-nomination-report">
                    <colgroup>
                        <col className="order" />
                        <col className="nominee" />
                        <col className="team" />
                        <col className="vote" />
                        <col className="nominator" />
                        <col className="status" />
                    </colgroup>
                    <thead>
                        <tr>
                            <th className="order">{getResource('SymbolCount')}</th>
                            <th className="nominee">{getResource('Nominee')}</th>
                            <th className="team">{getResource('Team')}</th>
                            <th className="vote">{getResource('Vote')}</th>
                            <th className="nominator">{getResource('Nominator')}</th>
                            <th className="status">{getResource('Status')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {content}
                    </tbody>
                </table>
            </Fragment>

        );
    }
}