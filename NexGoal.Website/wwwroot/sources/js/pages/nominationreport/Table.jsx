import React, { Component, Fragment } from 'react';

import getResource from '../../resourceHelper';
import { NominationStatuses } from '../../constants';
import { getNominationClassName, noDataIndication } from '../../common-functions';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nominationStatuses: NominationStatuses.map(x => ({
                value: x.value,
                label: x.text
            }))
        };
    }

    componentDidMount() {
        let header = this.refs.stickyHeader;

        window.addEventListener('scroll', function (e) {
            if (window.scrollY > 236) {
                header.classList.add('sticky');
            } else {
                header.classList.remove('sticky');
            }
        });
    }

    handleShowNominatorComment = (nomination, performanceRecordId) => {
        this.props.onShowNominatorComment(nomination, performanceRecordId);
    }

    handleChangeStatus = (nominationId, status) => {
        this.props.onUpdateStatus(nominationId, status.value);
    }

    renderNomination = (nomination, index) => {
        return (
            <tr key={nomination.nominationId}>
                <td className="text-center">{index + 1}</td>
                <td>{`${nomination.nominee.nickName} (${nomination.nominee.fullName})`}</td>
                <td>{nomination.nominee.groupName}</td>
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
                <td>
                    {<span className={getNominationClassName(nomination.statusId)}>
                        {NominationStatuses.find(x => x.value === nomination.statusId).text}
                    </span>}
                </td>
            </tr>
        );
    }

    render() {
        let content = this.props.tableData.length > 0 ? this.props.tableData.map(this.renderNomination)
            : <tr><td className="text-center" colSpan={6}>{noDataIndication()}</td></tr>;

        return (
            <table className="table table-bordered table-striped tbl-nomination-report">
                <colgroup>
                    <col className="order" />
                    <col className="nominee" />
                    <col className="team" />
                    <col className="nominator" />
                    <col className="status" />
                </colgroup>
                <thead>
                    <tr ref='stickyHeader'>
                        <th className="order">{getResource('SymbolCount')}</th>
                        <th className="nominee">{getResource('Nominee')}</th>
                        <th className="team">{getResource('Team')}</th>
                        <th className="nominator">{getResource('Nominator')}</th>
                        <th className="status">{getResource('Status')}</th>
                    </tr>
                </thead>
                <tbody>
                    {content}
                </tbody>
            </table>
        );
    }
}