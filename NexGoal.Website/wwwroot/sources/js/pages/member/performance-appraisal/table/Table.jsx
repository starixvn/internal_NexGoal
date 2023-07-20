import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import isEqual from 'react-fast-compare';

import { axiosError, resolveClientUrl, noDataIndication } from '../../../../common-functions';
import { PostRequestHeaders } from '../../../../constants';
import getResource from '../../../../resourceHelper';
import Rows from './Rows';
import EditMemberPA from '../EditPAForm';

class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowEditModal: false,
            paRecordId: null
        };
    }

    shouldComponentUpdate(nextProps, nextState) {
        return !isEqual(this.state, nextState) || !isEqual(this.props, nextProps);
    }

    onQuickEditPA = (paRecordId, isRevised) => {
        axios({
            url: resolveClientUrl('Review/QuickEdit'),
            method: 'POST',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                paRecordId: paRecordId,
                isRevised: !isRevised
            }),
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;
            if (data.isSuccessful) {
                this.props.onRevisedSuccess(paRecordId, isRevised);
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    };

    onClickEditPA = paRecordId => {
        this.setState({
            isShowEditModal: true,
            paRecordId: paRecordId
        });
    };

    onCloseModal = () => {
        this.setState({
            isShowEditModal: false,
            paRecordId: null
        });
    };

    onEditSucess = () => {
        this.onCloseModal();
        this.props.renderPA();
    };

    renderRow = data => {
        return (
            data.map(group => {
                return (
                    <Rows key={group.groupName}
                        data={group}
                        colSpan={7}
                        gradeOptions={this.props.gradeOptions}
                        onClickEditPA={this.onClickEditPA}
                        onQuickEditPA={this.onQuickEditPA}
                    />
                );
            })
        );
    };

    render() {
        let content = '';
        if (this.props.data) {
            content = this.props.data.length > 0 ? this.renderRow(this.props.data)
                : <tr><td className="text-center" colSpan={10}>{noDataIndication()}</td></tr>;
        }

        return (
            <Fragment>
                <table className="table table-bordered" >
                    <thead>
                        <tr>
                            <th rowSpan="2">{getResource('NickName')}</th>
                            <th rowSpan="2">{getResource('Toeic')}</th>
                            <th rowSpan="1" colSpan="4">{getResource('PerformanceRecord')}</th>

                            <th rowSpan="2">{getResource('PAFinal')}</th>
                            <th rowSpan="2">{getResource('Revised')}</th>
                        </tr>
                        <tr>
                            <th rowSpan="1">{getResource('Self')}</th>
                            <th rowSpan="1">{getResource('ProjectManager')}</th>
                            <th rowSpan="1">{getResource('Evaluator')}</th>
                            <th rowSpan="1">{getResource('Reviewer')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {content}
                    </tbody>
                </table>
                {
                    this.state.isShowEditModal &&
                    <EditMemberPA
                        isShowModal={this.state.isShowEditModal}
                        paRecordId={this.state.paRecordId}
                        gradeOptions={this.props.gradeOptions}
                        onCloseModal={this.onCloseModal}
                        onSubmitSuccess={this.onEditSucess}
                        performanceGradeInfos={this.props.performanceGradeInfos}
                    />
                }
            </Fragment>
        );
    }
}

export default Table;