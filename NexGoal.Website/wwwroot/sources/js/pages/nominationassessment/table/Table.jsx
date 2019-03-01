import React, { Component, Fragment } from 'react';
import Select from 'react-select';

import getResource from '../../../resourceHelper';
import { NominationStatuses } from '../../../constants';
import { getNominationClassName, noDataIndication } from '../../../common-functions';
import SingleValue from './SingleValue';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showStickyHeader: false
        };
    }

    componentDidMount() {
        window.addEventListener('scroll', e => {
            if (window.scrollY > 236) {
                if (!this.state.showStickyHeader) {
                    this.setState({ showStickyHeader: true });
                }
            } else {
                if (this.state.showStickyHeader) {
                    this.setState({ showStickyHeader: false });
                }
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
        let nominationStatusOptions = NominationStatuses.map(x => ({
            value: x.value,
            label: x.text
        }));
        let renderStatus = this.props.isHost ?
            <Select name="nominationPeriod" className="react-select-container" classNamePrefix="react-select"
                value={nominationStatusOptions.find(x => x.value === nomination.statusId)}
                options={nominationStatusOptions}
                isClearable={false} isSearchable={false}
                components={{ SingleValue }}
                onChange={this.handleChangeStatus.bind(this, nomination.nominationId)}
                menuPlacement="auto"
            />
            :
            <span className={getNominationClassName(nomination.statusId)}>
                {NominationStatuses.find(x => x.value === nomination.statusId).text}
            </span>;

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
                <td>{renderStatus}</td>
            </tr>
        );
    }

    render() {
        let content = this.props.tableData.length > 0 ? this.props.tableData.map(this.renderNomination)
            : <tr><td className="text-center" colSpan={6}>{noDataIndication()}</td></tr>;

        return (
            <Fragment>
                <div className={`sticky-block ${this.state.showStickyHeader ? '' : 'hidden'}`}>
                    <table className="table table-bordered table-striped tbl-nomination-assessment">
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
                </div>
                <table className="table table-bordered table-striped tbl-nomination-assessment">
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